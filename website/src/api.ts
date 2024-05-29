import moment from "moment";
import { Cpu } from "@/models/Cpu";
import { Memory } from "@/models/Memory";
import { Disk } from "@/models/Disk";

export class Api {
  static async executeQuery(queryString: string) {
    try{
      const response = await fetch("http://localhost:3000/graphql", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Accept: "application/json"
        },
        body: JSON.stringify({
          query: queryString
        })
      });

      return await response.json()
    }catch{
      return null
    }
  }

  private static dateToString(date: Date){
    if(date === null) return "null"
    return `"${moment(date).format("yyyy-MM-DD HH:mm")}"`
  }

  static async getCpu(startDate: Date, endDate: Date) {
    let startDateString = this.dateToString(startDate)
    let endDateString = this.dateToString(endDate)
    const queryString = `
    { 
      cpu(serverId: 0, startDateTime: ${startDateString}, endDateTime: ${endDateString}) {
        serverId,
        name,
        architecture,
        cores,
        threads,
        frequency,
        logs{
          date
          usage
          numberOfTasks
        }
      } 
    }`

    let response = await this.executeQuery(queryString)
    console.log(queryString)
    console.log("cpu response: ")
    console.log(response)
    if(response == null || response.data.cpu == null) return null
    let cpu = response.data.cpu
    return new Cpu(cpu.serverId, cpu.name, cpu.architecture, cpu.cores, cpu.threads, cpu.frequency, cpu.logs)
  }

  static async getMemory(startDate: Date, endDate: Date){
    let startDateString = this.dateToString(startDate)
    let endDateString = this.dateToString(endDate)
    const queryString = `
    {
      memory(serverId: 0, startDateTime: ${startDateString}, endDateTime: ${endDateString}) {
        serverId,
        logs{
          date, 
          totalKb,
          usedKb,
          freeKb,
          swapTotalKb,
          swapUsedKb,
          swapFreeKb,
          availableKb,
          cachedKb
        }
      }
    }`

    let response = await this.executeQuery(queryString)
    console.log(queryString)
    console.log("memory response:")
    console.log(response)
    if(response == null || response.data.memory == null) return null
    let memory = response.data.memory
    return new Memory(memory.serverId, memory.logs)
  }

  static async getDisks(startDate: Date, endDate: Date){
    let startDateString = this.dateToString(startDate)
    let endDateString = this.dateToString(endDate)
    const queryString = `
    {
      disk(serverId: 0, startDateTime: ${startDateString}, endDateTime: ${endDateString}) {
        serverId,
        uuid,
        type,
        serial,
        path,
        vendor,
        model,
        bytesTotal,
        partitions{
          uuid, label, filesystemName, filesystemVersion, mountPath,
          logs{
            date,
            bytes,
            usedPercentage
          }
        }
      }
    }`

    let response = await this.executeQuery(queryString)
    console.log(queryString)
    console.log("disk response:")
    console.log(response)
    if (response == null || response.data.disk == null) return []
    let disks = response.data.disk
    let result: Disk[] = []
    disks.forEach((d: any) => result.push(new Disk(d.serverId, d.uuid, d.type, d.serial, d.path, d.vendor, d.model, d.bytesTotal, d.partitions)))
    return result
  }
}

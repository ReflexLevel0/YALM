import moment from "moment";
import { Cpu } from "@/models/Cpu";
import { Memory } from "@/models/Memory";

export class Api {
  static async executeQuery(queryString: string) {
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
    let memory = response.data.memory
    return new Memory(memory.serverId, memory.logs)
  }
}

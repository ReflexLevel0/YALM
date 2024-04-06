import moment from "moment";
import { Cpu } from "@/models/Cpu";

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
    if(date === null) return ""
    return moment(date).format("yyyy-MM-DD HH:mm")
  }

  static async getCpuData(startDate: Date, endDate: Date) {
    let startDateString = startDate == null ? "null" : `"${this.dateToString(startDate)}"`
    let endDateString = endDate == null ? "null" : `"${this.dateToString(endDate)}"`
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
    }`;

    let response = await this.executeQuery(queryString)
    let cpu = response.data.cpu
    return new Cpu(cpu.serverId, cpu.name, cpu.architecture, cpu.cores, cpu.threads, cpu.frequency, cpu.logs)
  }
}

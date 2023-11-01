import { Api } from "@/api";
import { CpuLog } from "@/models/CpuLog";

export class ChartHelper {
  static DateToString(date: Date){
    return date.setMinutes(date.getMinutes() - date.getTimezoneOffset())
  }

  static async GetCpuUsageDataset(startDate: Date, endDate: Date) {
    let chartData: object;
    return Api.getCpuUsage(startDate, endDate)
      .then((r) => r.json())
      .then((json) => {
        const points: object[] = [];
        const cpuLogs: CpuLog[] = json.data.cpu;
        cpuLogs.forEach((log) => {
          points.push({
            x: this.DateToString(new Date(Date.parse(log.date))),
            y: log.usage * 100
          });
        });

        chartData = {
          datasets: [
            {
              showLine: true,
              label: "usage",
              borderColor: "#00ff04",
              backgroundColor: "#00ff04",
              data: points
            }
          ]
        };
        return chartData;
      });
  }

  static async GetCpuNumberOfTasksDataset(startDate: Date, endDate: Date) {
    let chartData: object;
    return Api.getCpuNumberOfTasks(startDate, endDate)
      .then((r) => r.json())
      .then((json) => {
        const points: object[] = [];
        const cpuLogs: CpuLog[] = json.data.cpu;
        cpuLogs.forEach((log) => {
          points.push({
            x: this.DateToString(new Date(Date.parse(log.date))),
            y: log.numberOfTasks
          });
        });

        chartData = {
          datasets: [
            {
              showLine: true,
              label: "number of tasks",
              borderColor: "#7acbf9",
              backgroundColor: "#7acbf9",
              data: points
            }
          ]
        };
        return chartData;
      });
  }
}

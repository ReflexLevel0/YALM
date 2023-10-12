import { Api } from "@/api";
import { CpuLog } from "@/models/CpuLog";
export class ChartHelper {
  static async GetCpuDatasets(startDate: Date, endDate: Date) {
    let chartData: object;
    return Api.getCpuLogs(startDate, endDate)
      .then((r) => r.json())
      .then((json) => {
        const numberOfTasksPoints: object[] = [];
        const usagePoints: object[] = [];
        const cpuLogs: CpuLog[] = json.data.cpu;
        cpuLogs.forEach((log) => {
          console.log(log)
          numberOfTasksPoints.push({ x: log.date, y: log.numberOfTasks });
          usagePoints.push({ x: log.date, y: log.usage });
        });

        chartData = {
          datasets: [
            {
              showLine: true,
              label: "number of tasks",
              borderColor: "#7acbf9",
              backgroundColor: "#7acbf9",
              data: numberOfTasksPoints,
            },
            {
              showLine: true,
              label: "usage",
              borderColor: "#00ff04",
              backgroundColor: "#00ff04",
              data: usagePoints,
            },
          ],
        };
        return chartData;
      });
  }
}

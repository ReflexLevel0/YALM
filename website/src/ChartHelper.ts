import { CpuLog } from "@/models/CpuLog";

export class ChartHelper {
  static CpuLogsToCpuUsageDataset(logs: CpuLog[]) {
    let points: object[] = [];
    logs.forEach(log => {
      points.push({
        x: log.date,
        y: log.usage * 100
      });
    });

    return {
      datasets: [
        {
          showLine: true,
          label: "usage",
          borderColor: "#00ff04",
          backgroundColor: "#00ff04",
          data: points
        }
      ]
    }
  }

  static CpuLogsToNumberOfTasksDataset(logs: CpuLog[]) {
    let points: object[] = [];

    logs.forEach((log) => {
      points.push({
        x: log.date,
        y: log.numberOfTasks == null ? 0 : log.numberOfTasks
      });
    });

    return {
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
  }
}

import { CpuLog } from "@/models/CpuLog";
import { MemoryLog } from "@/models/MemoryLog";
import { PartitionLog } from "@/models/PartitionLog";

export class ChartHelper {
  static CpuLogsToCpuUsageDataset(logs: CpuLog[]) {
    if (logs == null) return { datasets: [] }

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
    if (logs == null) return { datasets: [] }

    let points: object[] = []
    logs.forEach((log) => {
      points.push({
        x: log.date,
        y: log.numberOfTasks == null ? 0 : log.numberOfTasks
      })
    })

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
    }
  }

  static MemoryLogsToDataset(logs: MemoryLog[]){
    if (logs == null) return {datasets: []}

    let memoryPoints: object[] = [];
    let swapMemoryPoints: object[] = [];
    let cachedPoints: object[] = [];
    logs.forEach(log => {
      memoryPoints.push({
        x: log.date,
        y: log.usedKb == null || log.totalKb == null ? 0 : log.usedKb / log.totalKb * 100
      })
      swapMemoryPoints.push({
        x: log.date,
        y: log.swapUsedKb == null || log.swapTotalKb == null ? 0 : log.swapUsedKb / log.swapTotalKb * 100
      })
      cachedPoints.push({
        x: log.date,
        y: log.cachedKb == null || log.totalKb == null ? 0 : log.cachedKb / log.totalKb * 100
      })
    })

    return {
      datasets: [
        {
          showLine: true,
          label: "memory used %",
          borderColor: "#fa1818",
          backgroundColor: "#fa1818",
          data: memoryPoints
        },
        {
          showLine: true,
          label: "swap memory used %",
          borderColor: "#65e142",
          backgroundColor: "#65e142",
          data: swapMemoryPoints
        },
        {
          showLine: true,
          label: "cached memory %",
          borderColor: "#b25be8",
          backgroundColor: "#b25be8",
          data: cachedPoints
        },
      ]
    };
  }

  static PartitionLogsToDataset(logs: PartitionLog[]){
    if (logs == null) return {datasets: []}

    let usagePoints: object[] = []
    logs.forEach(l => {
      usagePoints.push({
        x: l.date,
        y: l.usedPercentage == null ? 0 : l.usedPercentage * 100
      })
    })

    return {
      datasets: [
        {
          showLine: true,
          label: "disk usage %",
          borderColor: "#1a881c",
          backgroundColor: "#1a881c",
          data: usagePoints
        }
      ]
    }
  }
}

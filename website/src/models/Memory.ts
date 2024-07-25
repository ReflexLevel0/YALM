import { MemoryLog } from "@/models/MemoryLog";

export class Memory {
  serverId: number;
  logs: MemoryLog[];

  constructor(serverId: number, logs: MemoryLog[]) {
    this.serverId = serverId
    this.logs = logs
  }
}
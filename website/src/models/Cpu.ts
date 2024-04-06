import { CpuLog } from "@/models/CpuLog";

export class Cpu {
  serverId: number;
  name: string;
  architecture: string;
  cores: number;
  threads: number;
  frequency: number;
  logs: CpuLog [];

  constructor(serverId: number, name: string, architecture: string, cores: number, threads: number, frequency: number, logs: CpuLog[]) {
    this.serverId = serverId
    this.name = name
    this.architecture = architecture
    this.cores = cores
    this.threads = threads
    this.frequency = frequency
    this.logs = logs
  }
}

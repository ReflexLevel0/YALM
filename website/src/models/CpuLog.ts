export class CpuLog {
  date: string;
  usage: number;
  numberOfTasks: number;

  constructor(date: string, usage: number, numberOfTasks: number) {
    this.date = date;
    this.usage = usage;
    this.numberOfTasks = numberOfTasks;
  }
}

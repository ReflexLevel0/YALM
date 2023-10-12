export class CpuLog {
  date: Date;
  usage: number;
  numberOfTasks: number;

  constructor(date: Date, usage: number, numberOfTasks: number) {
    this.date = date;
    this.usage = usage;
    this.numberOfTasks = numberOfTasks;
  }
}

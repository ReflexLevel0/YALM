export class Alert {
  serverId: number;
  date: Date;
  severity: number;
  text: string;

  constructor(serverId: number, date: Date, severity: number, text: string) {
    this.serverId = serverId
    this.date = date
    this.severity = severity
    this.text = text
  }
}
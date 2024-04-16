export class PartitionLog {
  date: Date
  bytes: number
  usedPercentage: number

  constructor(date: Date, bytes: number, usedPercentage: number) {
    this.date = date
    this.bytes = bytes
    this.usedPercentage = usedPercentage
  }
}
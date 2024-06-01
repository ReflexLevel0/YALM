export class PartitionLog {
  date: string
  bytes: number
  usedPercentage: number

  constructor(date: string, bytes: number, usedPercentage: number) {
    this.date = date
    this.bytes = bytes
    this.usedPercentage = usedPercentage
  }
}
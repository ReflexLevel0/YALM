export class MemoryLog{
  date: string;
  totalKb: number;
  usedKb: number;
  freeKb: number;
  availableKb: number;
  cachedKb: number;
  swapTotalKb: number;
  swapUsedKb: number;
  swapFreeKb: number;

  constructor(date: string, totalKb: number, usedKb: number, freeKb: number, availableKb: number, cachedKb: number, swapTotalKb: number, swapUsedKb: number, swapFreeKb: number) {
    this.date = date
    this.totalKb = totalKb
    this.usedKb = usedKb
    this.freeKb = freeKb
    this.availableKb = availableKb
    this.cachedKb = cachedKb
    this.swapTotalKb = swapTotalKb
    this.swapUsedKb = swapUsedKb
    this.swapFreeKb = swapFreeKb
  }

}
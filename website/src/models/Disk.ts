import { Partition } from "@/models/Partition"

export class Disk {
  serverId: number;
  uuid: string;
  type: string;
  serial: string;
  path: string;
  vendor: string;
  model: string;
  bytesTotal: number;
  partitions: Partition [];

  constructor(serverId: number, uuid: string, type: string, serial: string, path: string, vendor: string, model: string, bytesTotal: number, partitions: Partition []){
    this.serverId = serverId
    this.uuid = uuid
    this.type = type
    this.serial = serial
    this.path = path
    this.vendor = vendor
    this.model = model
    this.bytesTotal = bytesTotal
    this.partitions = partitions
  }
}
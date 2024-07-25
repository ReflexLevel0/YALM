import { PartitionLog } from "@/models/PartitionLog"

export class Partition {
  uuid: string
  label: string
  filesystemName: string
  filesystemVersion: string
  mountPath: string
  logs: PartitionLog[]

  constructor(uuid: string, label: string, filesystemName: string, filesystemVersion: string, mountPath: string, logs: PartitionLog[]){
    this.uuid = uuid
    this.label = label
    this.filesystemName = filesystemName
    this.filesystemVersion = filesystemVersion
    this.mountPath = mountPath
    this.logs = logs
  }
}
export class Server {
  serverId: number
  status: string

  constructor(serverId: number, status: string) {
    this.serverId = serverId
    this.status = status
  }
}
export class Api {
  static executeQuery(queryString: string) {
    return fetch("http://localhost:3000/graphql", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Accept: "application/json",
      },
      body: JSON.stringify({
        query: queryString,
      }),
    });
  }

  static getCpuLogs() {
    return this.executeQuery(
      '{ cpu(serverId: 0, interval: 480, method: "avg"){date, numberOfTasks, usage} }'
    );
  }
}

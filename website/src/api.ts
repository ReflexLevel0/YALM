import moment from "moment";

export class Api {
  static executeQuery(queryString: string) {
    return fetch("http://localhost:3000/graphql", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Accept: "application/json"
      },
      body: JSON.stringify({
        query: queryString
      })
    });
  }

  static getCpuLogs(startDate: Date, endDate: Date) {
    const startDateString = startDate === null ? "" : moment(startDate).format("yyyy-MM-DD HH:mm:ss");
    const endDateString = endDate === null ? "" : moment(endDate).format("yyyy-MM-DD HH:mm:ss");
    const queryString = `{ cpu(serverId: 0, startDateTime: "${startDateString}", endDateTime: "${endDateString}") {date, numberOfTasks, usage} }`;
    return this.executeQuery(queryString);
  }
}

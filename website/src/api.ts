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
    const startDateString = moment(startDate).format("yyyy-MM-DD HH:mm:ss");
    const endDateString = moment(endDate).format("yyyy-MM-DD HH:mm:ss");
    const queryString = `{ cpu(serverId: 0, interval: 60, method: "avg", startDateTime: "${startDateString}" endDateTime: "${endDateString}") {date, numberOfTasks, usage} }`;
    console.log(queryString)
    return this.executeQuery(queryString);
  }
}

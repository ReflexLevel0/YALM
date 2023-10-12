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
    const dateDiff = moment(endDate).diff(startDate, 'hours')
    const queryString = `{ cpu(serverId: 0, interval: ${dateDiff}, method: "avg", startDateTime: "${startDateString}" endDateTime: "${endDateString}") {date, numberOfTasks, usage} }`;
    return this.executeQuery(queryString);
  }
}

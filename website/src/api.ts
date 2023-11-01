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

  private static dateToString(date: Date){
    if(date === null) return ""
    date.setMinutes(date.getMinutes() + date.getTimezoneOffset())
    return moment(date).format("yyyy-MM-DD HH:mm:ss")
  }

  static getCpuUsage(startDate: Date, endDate: Date) {
    const queryString = `
    { 
      cpu(serverId: 0, startDateTime: "${this.dateToString(startDate)}", endDateTime: "${this.dateToString(endDate)}") {
        date, 
        usage
      } 
    }`;
    console.log(queryString)
    return this.executeQuery(queryString);
  }

  static getCpuNumberOfTasks(startDate: Date, endDate: Date){
    const queryString = `
    { 
      cpu(serverId: 0, startDateTime: "${this.dateToString(startDate)}", endDateTime: "${this.dateToString(endDate)}") {
        date, 
        numberOfTasks
      } 
    }`;
    return this.executeQuery(queryString);
  }
}

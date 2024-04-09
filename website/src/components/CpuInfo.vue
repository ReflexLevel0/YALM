<script>
import { Cpu } from "@/models/Cpu";
import { Api } from "@/api";
import Chart from "@/components/Chart.vue";
import { ChartHelper } from "@/ChartHelper";

export default {
  computed: {
    Api() {
      return Api
    }
  },
  data() {
    return {
      cpu: Cpu,
      cpuUsageChartData: null,
      numberOfTasksChartData: null,
      cpuUsageChartConfig: {
        startDate: null,
        endDate: null
      },
      numberOfTasksChartConfig: {
        startDate: null,
        endDate: null
      }
    };
  },
  props: {
    startDate: null,
    endDate: null
  },
  components: {
    Chart
  },
  methods: {
    //Refreshing all CPU data
    async refreshData(startDate, endDate) {
      let cpu = await Api.getCpu(startDate, endDate)
      this.$data.cpu = cpu
      await this.refreshCpuUsageChart(cpu.logs)
      await this.refreshNumberOfTasksChart(cpu.logs)
    },

    //Refreshing only CPU usage chart with provided logs (fetching logs if no logs are provided)
    async refreshCpuUsageChart(logs = null) {
      if(logs === null){
        let cpu = await Api.getCpu(this.$data.cpuUsageChartConfig.startDate, this.$data.cpuUsageChartConfig.endDate)
        logs = cpu.logs
      }
      this.$data.cpuUsageChartData = ChartHelper.CpuLogsToCpuUsageDataset(logs)
    },

    //Refreshing only number of tasks chart with provided logs (fetching logs if no logs are provided)
    async refreshNumberOfTasksChart(logs = null) {
      if(logs === null){
        let cpu = await Api.getCpu(this.$data.numberOfTasksChartConfig.startDate, this.$data.numberOfTasksChartConfig.endDate)
        logs = cpu.logs
      }
      this.$data.numberOfTasksChartData = ChartHelper.CpuLogsToNumberOfTasksDataset(logs);
    }
  },
  async mounted() {
    await this.refreshData(this.$props.startDate, this.$props.endDate);
  },
  watch: {

    //Refreshing data when used picks a new start date
    "startDate": async function() {
      await this.refreshData(this.$props.startDate, this.$props.endDate);
    },

    //Refreshing data when used picks a new end date
    "endDate": async function() {
      await this.refreshData(this.$props.startDate, this.$props.endDate);
    }
  }
};
</script>

<template>
  <!-- Displaying generic CPU data if it has been loaded -->
  <div v-if="cpu !== null">
    <p><b>{{ $data.cpu.name }}</b></p>
    <ul>
      <li>Architecture: {{ $data.cpu.architecture }}</li>
      <li>Cores: {{ $data.cpu.cores }}</li>
      <li>Threads: {{ $data.cpu.threads }}</li>
      <li>Frequency: {{ $data.cpu.frequency }}MHz</li>
    </ul>
  </div>

  <!-- Chart showing CPU usage over time -->
  <Chart
    name="CPU usage"
    :scales="{ x: { type: 'time' }, y: { min: 0, max: 100 } }"
    :chart-data="this.$data.cpuUsageChartData"
    @zoom-changed="async (limits) =>
      {
        $data.cpuUsageChartConfig.startDate = limits.startDate == null ? $props.startDate : limits.startDate;
        $data.cpuUsageChartConfig.endDate = limits.endDate == null ? $props.endDate : limits.endDate;
        await this.refreshCpuUsageChart()
      }"
  />

  <!-- Chart showing number of tasks over time -->
  <Chart
    name="Number of tasks"
    :scales="{ x: { type: 'time' }, y: { min: 0 } }"
    :chart-data="this.$data.numberOfTasksChartData"
    @zoom-changed="async (limits) =>
        {
          $data.numberOfTasksChartConfig.startDate = limits.startDate == null ? $props.startDate : limits.startDate;
          $data.numberOfTasksChartConfig.endDate = limits.endDate == null ? $props.endDate : limits.endDate
          await this.refreshNumberOfTasksChart()
        }"
  />
</template>
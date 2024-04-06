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
        endDate: null,
        reloadingData: true
      },
      numberOfTasksChartConfig: {
        startDate: null,
        endDate: null,
        reloadingData: true
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
    async refreshData() {
      this.$data.cpuUsageChartConfig.reloadingData = true
      this.$data.numberOfTasksChartConfig.reloadingData = true
      let cpu = await Api.getCpu(this.$props.startDate, this.$props.endDate)
      this.$data.cpu = cpu
      await this.refreshCpuUsageChart(cpu.logs)
      await this.refreshNumberOfTasksChart(cpu.logs)
      this.$data.cpuUsageChartConfig.reloadingData = false
      this.$data.numberOfTasksChartConfig.reloadingData = false
    },

    //Refreshing only CPU usage chart with provided logs (fetching logs if no logs are provided)
    async refreshCpuUsageChart(logs = null) {
      this.$data.cpuUsageChartConfig.reloadingData = true
      if(logs === null){
        let cpu = await Api.getCpu(this.$data.cpuUsageChartConfig.startDate, this.$data.cpuUsageChartConfig.endDate)
        logs = cpu.logs
      }
      this.$data.cpuUsageChartData = ChartHelper.CpuLogsToCpuUsageDataset(logs)
      this.$data.cpuUsageChartConfig.reloadingData = false
    },

    //Refreshing only number of tasks chart with provided logs (fetching logs if no logs are provided)
    async refreshNumberOfTasksChart(logs = null) {
      this.$data.numberOfTasksChartConfig.reloadingData = true
      if(logs === null){
        let cpu = await Api.getCpu(this.$data.numberOfTasksChartConfig.startDate, this.$data.numberOfTasksChartConfig.endDate)
        logs = cpu.logs
      }
      this.$data.numberOfTasksChartData = ChartHelper.CpuLogsToNumberOfTasksDataset(logs);
      this.$data.numberOfTasksChartConfig.reloadingData = false
    }
  },
  async mounted() {
    await this.refreshData();
  },
  watch: {
    //Refreshing all data when start date has changed
    "startDate": async function() {
      await this.refreshData();
    },

    //Refreshing all data when end date has changed
    "endDate": async function() {
      await this.refreshData();
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
    :chart-data="numberOfTasksChartData"
    @zoom-changed="async (limits) =>
        {
          $data.numberOfTasksChartConfig.startDate = limits.startDate == null ? $props.startDate : limits.startDate;
          $data.numberOfTasksChartConfig.endDate = limits.endDate == null ? $props.endDate : limits.endDate
          await this.refreshNumberOfTasksChart()
        }"
  />
</template>
<script>
import { Cpu } from "@/models/Cpu";
import { Api } from "@/api";
import Chart from "../components/Chart.vue";
import { ChartHelper } from "@/ChartHelper";

export default {
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
    async refreshCpu() {
      this.$data.cpuUsageChartConfig.reloadingData = true;
      this.$data.numberOfTasksChartConfig.reloadingData = true;

      let data = await Api.getCpuData(this.$props.startDate, this.$props.endDate);
      let cpu = data.data.cpu;
      this.$data.cpu = new Cpu(cpu.serverId, cpu.name, cpu.architecture, cpu.cores, cpu.threads, cpu.frequency, cpu.logs);

      this.$data.cpuUsageChartData = ChartHelper.CpuLogsToCpuUsageDataset(cpu.logs);
      this.$data.numberOfTasksChartData = ChartHelper.CpuLogsToNumberOfTasksDataset(cpu.logs);

      this.$data.cpuUsageChartConfig.reloadingData = false;
      this.$data.numberOfTasksChartConfig.reloadingData = false;
    },

    async refreshCpuUsageChart() {
      let data = await Api.getCpuData(this.cpuUsageChartConfig.startDate, this.cpuUsageChartConfig.endDate);
      let logs = data.data.cpu.logs;
      this.$data.cpuUsageChartData = ChartHelper.CpuLogsToCpuUsageDataset(logs);
    },

    async refreshNumberOfTasksChart() {
      let data = await Api.getCpuData(this.numberOfTasksChartConfig.startDate, this.numberOfTasksChartConfig.endDate);
      let logs = data.data.cpu.logs;
      this.$data.numberOfTasksChartData = ChartHelper.CpuLogsToNumberOfTasksDataset(logs);
    }
  },
  async mounted() {
    await this.refreshCpu();
  },
  watch: {
    "startDate": async function() {
      await this.refreshCpu();
    },
    "endDate": async function() {
      await this.refreshCpu();
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

<style scoped>

</style>
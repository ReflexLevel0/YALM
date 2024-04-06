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
    async refreshData() {
      let cpu = await Api.getCpuData(this.$props.startDate, this.$props.endDate);
      this.$data.cpu = cpu
      await this.refreshCpuUsageChart(cpu.logs)
      await this.refreshNumberOfTasksChart(cpu.logs)
    },

    async refreshCpuUsageChart(logs) {
      this.$data.cpuUsageChartConfig.reloadingData = true;
      this.$data.cpuUsageChartData = ChartHelper.CpuLogsToCpuUsageDataset(logs);
      this.$data.cpuUsageChartConfig.reloadingData = false;
    },

    async refreshNumberOfTasksChart(logs) {
      this.$data.numberOfTasksChartConfig.reloadingData = true;
      this.$data.numberOfTasksChartData = ChartHelper.CpuLogsToNumberOfTasksDataset(logs);
      this.$data.numberOfTasksChartConfig.reloadingData = false;
    }
  },
  async mounted() {
    await this.refreshData();
  },
  watch: {
    "startDate": async function() {
      await this.refreshData();
    },
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
        let cpu = await Api.getCpuData($data.cpuUsageChartConfig.startDate, $data.cpuUsageChartConfig.endDate)
        await this.refreshCpuUsageChart(cpu.logs)
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
          let cpu = await Api.getCpuData($data.numberOfTasksChartConfig.startDate, $data.numberOfTasksChartConfig.endDate)
          await this.refreshNumberOfTasksChart(cpu.logs)
        }"
  />
</template>
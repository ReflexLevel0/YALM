<script>
import { Cpu } from "@/models/Cpu";
import { Api } from "@/api";
import Chart from "@/components/Chart.vue";
import { ChartHelper } from "@/ChartHelper";
import Fieldset from "primevue/fieldset";

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
    endDate: null,
    serverId: {
      required: true
    }
  },
  components: {
    Chart,
    Fieldset
  },
  methods: {
    //Refreshing all CPU data
    async refreshData() {
      this.$data.cpu = await Api.getCpu(this.$props.serverId, this.$props.startDate, this.$props.endDate)
      await this.refreshCpuUsageChart(false)
      await this.refreshNumberOfTasksChart(false)
    },

    //Refreshing only CPU usage chart with provided logs (fetching logs if no logs are provided)
    async refreshCpuUsageChart(refresh) {
      if(refresh){
        this.$data.cpu = await Api.getCpu(this.$props.serverId, this.$data.cpuUsageChartConfig.startDate, this.$data.cpuUsageChartConfig.endDate)
      }
      this.$data.cpuUsageChartData = ChartHelper.CpuLogsToCpuUsageDataset(this.$data.cpu?.logs)
    },

    //Refreshing only number of tasks chart with provided logs (fetching logs if no logs are provided)
    async refreshNumberOfTasksChart(refresh) {
      if(refresh){
        this.$data.cpu = await Api.getCpu(this.$props.serverId, this.$data.numberOfTasksChartConfig.startDate, this.$data.numberOfTasksChartConfig.endDate)
      }
      this.$data.numberOfTasksChartData = ChartHelper.CpuLogsToNumberOfTasksDataset(this.$data.cpu?.logs);
    }
  },
  async mounted() {
    await this.refreshData();
  },
  watch: {

    //Refreshing data when used picks a new start date
    "startDate": async function() {
      await this.refreshData();
    },

    //Refreshing data when used picks a new end date
    "endDate": async function() {
      await this.refreshData();
    },

    //Refreshing data when serverId changes
    "serverId": async function() {
      await this.refreshData()
    }

  }
};
</script>

<template>
  <Fieldset legend="CPU" :toggleable="true">
    <!-- Displaying generic CPU data if it has been loaded -->
    <div v-if="cpu !== null">
      <p>{{ $data.cpu.name }}</p>
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
        await this.refreshCpuUsageChart(true)
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
          await this.refreshNumberOfTasksChart(true)
        }"
    />
  </Fieldset>
</template>
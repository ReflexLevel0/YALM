<script>
import Chart from "./components/Chart.vue";
import { ChartHelper } from "@/ChartHelper";
import VueDatePicker from "@vuepic/vue-datepicker";
import "@vuepic/vue-datepicker/dist/main.css";
import { nextTick } from "vue";

export default {
  name: "app",
  data() {
    return {
      startDate: null,
      endDate: null,
      cpuUsageConfig: {
        startDate: null,
        endDate: null,
        reloadingChart: false
      },
      cpuNumberOfTasksConfig: {
        startDate: null,
        endDate: null,
        reloadingChart: false
      }
    };
  },
  computed: {
    CpuUsageDatasetLoader(){
      return ChartHelper.GetCpuUsageDataset(
        this.$data.cpuUsageConfig.startDate,
        this.$data.cpuUsageConfig.endDate
      )
    },
    CpuNumberOfTasksDatasetLoader(){
      return ChartHelper.GetCpuNumberOfTasksDataset(
        this.$data.cpuNumberOfTasksConfig.startDate,
        this.$data.cpuNumberOfTasksConfig.endDate
      )
    }
  },
  components: {
    Chart,
    VueDatePicker,
  },
  watch: {
    'startDate': {
      handler: function(){
        this.refreshChartDates()
      }
    },
    'endDate': {
      handler: function(){
        this.refreshChartDates()
      }
    }
  },
  methods: {
    refreshChartDates(){
      this.refreshCpuUsageChartDates()
      this.refreshCpuNumberOfTasksChartDates()
    },
    refreshCpuUsageChartDates(){
      this.$data.cpuUsageConfig.reloadingChart = true
      nextTick(() => {
        this.$data.cpuUsageConfig.startDate = this.$data.startDate
        this.$data.cpuUsageConfig.endDate = this.$data.endDate
        this.$data.cpuUsageConfig.reloadingChart = false
      })
    },
    refreshCpuNumberOfTasksChartDates(){
      this.$data.cpuNumberOfTasksConfig.reloadingChart = true
      nextTick(() => {
        this.$data.cpuNumberOfTasksConfig.startDate = this.$data.startDate
        this.$data.cpuNumberOfTasksConfig.endDate = this.$data.endDate
        this.$data.cpuNumberOfTasksConfig.reloadingChart = false
      })
    }
  }
};
</script>
<template>
  <VueDatePicker v-model="startDate" />
  <VueDatePicker v-model="endDate" />
  <Chart
    v-if="cpuUsageConfig.reloadingChart === false"
    name="CPU"
    :scales="{ x: { type: 'time' }, y: { min: 0, max: 100 } }"
    :get-data-promise="CpuUsageDatasetLoader"
    @zoom-changed="(limits) =>
    {
      $data.cpuUsageConfig.startDate = limits.startDate;
      $data.cpuUsageConfig.endDate = limits.endDate
    }"
    @reload-chart="refreshCpuUsageChartDates"
  />
  <Chart
    v-if="cpuNumberOfTasksConfig.reloadingChart === false"
    name="CPU"
    :scales="{ x: { type: 'time' }, y: { min: 0 } }"
    :get-data-promise="CpuNumberOfTasksDatasetLoader"
    @zoom-changed="(limits) =>
    {
      $data.cpuNumberOfTasksConfig.startDate = limits.startDate;
      $data.cpuNumberOfTasksConfig.endDate = limits.endDate
    }"
    @reload-chart="refreshCpuNumberOfTasksChartDates"
  />
</template>

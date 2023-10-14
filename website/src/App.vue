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
      cpuConfig: {
        startDate: null,
        endDate: null,
        reloadingCpuChart: false
      },
    };
  },
  computed: {
    ChartHelper() {
      return ChartHelper;
    },
    CpuDatasetLoader(){
      return ChartHelper.GetCpuDatasets(this.$data.cpuConfig.startDate, this.$data.cpuConfig.endDate)
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
      this.$data.cpuConfig.reloadingCpuChart = true
      nextTick(() => {
        this.$data.cpuConfig.startDate = this.$data.startDate
        this.$data.cpuConfig.endDate = this.$data.endDate
        this.$data.cpuConfig.reloadingCpuChart = false
      })
    }
  }
};
</script>
<template>
  <VueDatePicker v-model="startDate" />
  <VueDatePicker v-model="endDate" />
  <Chart
    v-if="cpuConfig.startDate && cpuConfig.endDate && cpuConfig === false"
    name="CPU"
    :get-data-promise="CpuDatasetLoader"
    @zoom-changed="(limits) => $data.cpuConfig = limits"
    @reload-chart="refreshChartDates"
  />
</template>

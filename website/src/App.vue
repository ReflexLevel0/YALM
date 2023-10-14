<script>
import Chart from "./components/Chart.vue";
import { ChartHelper } from "@/ChartHelper";
import VueDatePicker from "@vuepic/vue-datepicker";
import "@vuepic/vue-datepicker/dist/main.css";

export default {
  name: "app",
  data() {
    return {
      startDate: null,
      endDate: null,
      cpuChartDates: {
        startDate: null,
        endDate: null
      }
    };
  },
  computed: {
    ChartHelper() {
      return ChartHelper;
    },
    CpuDatasetLoader(){
      return ChartHelper.GetCpuDatasets(this.$data.cpuChartDates.startDate, this.$data.cpuChartDates.endDate)
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
      this.$data.cpuChartDates.startDate = this.$data.startDate
      this.$data.cpuChartDates.endDate = this.$data.endDate
    }
  }
};
</script>
<template>
  <VueDatePicker v-model="startDate" />
  <VueDatePicker v-model="endDate" />
  <Chart
    v-if="cpuChartDates.startDate && cpuChartDates.endDate"
    name="CPU"
    :get-data-promise="CpuDatasetLoader"
    @zoom-changed="(limits) => $data.cpuChartDates = limits"
  />
</template>

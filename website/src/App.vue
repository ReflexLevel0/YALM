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
    };
  },
  computed: {
    ChartHelper() {
      return ChartHelper;
    },
  },
  components: {
    Chart,
    VueDatePicker,
  },
};
</script>
<template>
  <VueDatePicker v-model="startDate" />
  <VueDatePicker v-model="endDate" />
  <Chart
    v-if="startDate && endDate"
    name="CPU"
    :get-data-promise="ChartHelper.GetCpuDatasets(startDate, endDate)"
  />
</template>

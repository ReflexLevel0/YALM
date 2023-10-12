<script>
import Chart from "./components/Chart.vue";
import { ChartHelper } from "@/ChartHelper";

export default {
  name: "app",
  computed: {
    ChartHelper() {
      return ChartHelper;
    },
  },
  components: {
    Chart,
  },
};
</script>
<template>
  <Chart name="CPU" :get-data-promise="ChartHelper.GetCpuDatasets()" />
</template>

<script>
import Chart from "./components/Chart.vue";
import VueDatePicker from "@vuepic/vue-datepicker";
import "@vuepic/vue-datepicker/dist/main.css";
import CpuInfo from "@/components/CpuInfo.vue";

export default {
  name: "app",
  data() {
    return {
      startDate: null,
      endDate: null,
    };
  },
  components: {
    CpuInfo,
    Chart,
    VueDatePicker,
  },
  created() {
    //Setting start date to be a week ago by default
    let weekAgo = new Date(Date.now())
    weekAgo = new Date(weekAgo.setDate(weekAgo.getDate() - 7))
    this.$data.startDate = weekAgo
  }
};
</script>
<template>
  <VueDatePicker v-model="startDate" format="yyyy-MM-dd HH:mm" />
  <VueDatePicker v-model="endDate" format="yyyy-MM-dd HH:mm" />
  <CpuInfo :start-date="this.$data.startDate" :end-date="this.$data.endDate"/>
</template>

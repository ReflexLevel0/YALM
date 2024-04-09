<script>
import Chart from "./components/Chart.vue";
import VueDatePicker from "@vuepic/vue-datepicker";
import "@vuepic/vue-datepicker/dist/main.css";
import CpuInfo from "@/components/CpuInfo.vue";
import MemoryInfo from "@/components/MemoryInfo.vue";

export default {
  name: "app",
  data() {
    return {
      startDate: null,
      endDate: null,
    };
  },
  components: {
    MemoryInfo,
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
  <div class="summary">
    <CpuInfo :start-date="this.$data.startDate" :end-date="this.$data.endDate"/>
    <MemoryInfo :start-date="this.$data.startDate" :end-date="this.$data.endDate"/>
  </div>
</template>

<style scoped>
@media screen and (min-width: 1000px) {
  .summary{
    max-height: 100vh;
    display: flex;
    flex-flow: column wrap;
    //background-color: red;
  }
}
@media screen and (max-width: 999px) {
  .summary {
    display: flex;
    flex-flow: column nowrap;
    //background-color: green;
  }
}
</style>
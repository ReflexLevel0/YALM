<script>
import Chart from "../components/Chart.vue";
import VueDatePicker from "@vuepic/vue-datepicker";
import "@vuepic/vue-datepicker/dist/main.css";
import CpuInfo from "@/components/CpuInfo.vue";
import MemoryInfo from "@/components/MemoryInfo.vue";
import DiskInfo from "@/components/DiskInfo.vue";

export default {
  name: "app",
  data() {
    return {
      startDate: null,
      endDate: null,
      serverId: null,
    };
  },
  components: {
    DiskInfo,
    MemoryInfo,
    CpuInfo,
    Chart,
    VueDatePicker
  },
  created() {
    //Setting start date to be a week ago by default
    let weekAgo = new Date(Date.now())
    weekAgo = new Date(weekAgo.setDate(weekAgo.getDate() - 7))
    this.$data.startDate = weekAgo
    this.$data.serverId = this.$route.params["id"]
  },
  beforeRouteUpdate(to, from){
    this.$data.serverId = to.params["id"]
  }
};
</script>

<template>
  <VueDatePicker v-model="startDate" format="yyyy-MM-dd HH:mm" />
  <VueDatePicker v-model="endDate" format="yyyy-MM-dd HH:mm" />
  <h2>
    Server ID: {{this.$data.serverId}}
  </h2>
  <div class="summary">
    <CpuInfo :server-id="this.$data.serverId" :start-date="this.$data.startDate" :end-date="this.$data.endDate"/>
    <MemoryInfo :server-id="this.$data.serverId" :start-date="this.$data.startDate" :end-date="this.$data.endDate"/>
    <DiskInfo :server-id="this.$data.serverId" :start-date="this.$data.startDate" :end-date="this.$data.endDate"/>
  </div>
</template>

<style scoped>
.summary{
  margin-top: 20px;
}
@media screen and (min-width: 1000px) {
  .summary{
    max-height: 90vh;
    display: flex;
    flex-flow: column wrap;
  }
}
@media screen and (max-width: 999px) {
  .summary {
    display: flex;
    flex-flow: column nowrap;
  }
}
</style>
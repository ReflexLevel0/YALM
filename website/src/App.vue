<script>
import Chart from "./components/Chart.vue";
import VueDatePicker from "@vuepic/vue-datepicker";
import "@vuepic/vue-datepicker/dist/main.css";
import CpuInfo from "@/components/CpuInfo.vue";
import MemoryInfo from "@/components/MemoryInfo.vue";
import { VueCollapsiblePanel, VueCollapsiblePanelGroup } from '@dafcoe/vue-collapsible-panel'
import '@dafcoe/vue-collapsible-panel/dist/vue-collapsible-panel.css'
import DiskInfo from "@/components/DiskInfo.vue";

export default {
  name: "app",
  data() {
    return {
      startDate: null,
      endDate: null,
    };
  },
  components: {
    DiskInfo,
    MemoryInfo,
    CpuInfo,
    Chart,
    VueDatePicker,
    VueCollapsiblePanel,
    VueCollapsiblePanelGroup
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
  <VueCollapsiblePanelGroup class="summary">
    <VueCollapsiblePanel>
      <template #title>CPU</template>
      <template #content>
        ===
        <CpuInfo :start-date="this.$data.startDate" :end-date="this.$data.endDate"/>
      </template>
    </VueCollapsiblePanel>
    <VueCollapsiblePanel>
      <template #title>RAM</template>
      <template #content>
        ===
        <MemoryInfo :start-date="this.$data.startDate" :end-date="this.$data.endDate"/>
      </template>
    </VueCollapsiblePanel>
    <VueCollapsiblePanel>
      <template #title>Disks</template>
      <template #content>
        ===
        <DiskInfo :start-date="this.$data.startDate" :end-date="this.$data.endDate"/>
      </template>
    </VueCollapsiblePanel>
  </VueCollapsiblePanelGroup>
</template>

<style scoped>
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
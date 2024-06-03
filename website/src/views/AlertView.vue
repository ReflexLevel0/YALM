<script>
import { Api } from "@/api";
import DataTable from 'primevue/datatable';
import Column from 'primevue/column';
import 'primevue/resources/themes/lara-light-teal/theme.css'
import Tag from "primevue/tag";

export default {
  data(){
    return {
      loading: true,
      alerts: null
    }
  },
  async created() {
    this.$data.alerts = await Api.getAlerts()
    this.$data.loading = false
  },
  components: {
    DataTable,
    Column,
    Tag
  },
  methods: {
    getSeverity(severity){
      if(severity === "INFORMATION") return "info"
      else if(severity === "WARNING") return "warning"
      else if(severity === "CRITICAL") return "danger"
      return null
    }
  }
}
</script>

<template>
  <div v-if="loading" class="info">
    Loading...
  </div>

  <div v-else-if="alerts == null || alerts.length === 0" class="info">
    There are no alerts
  </div>

  <DataTable v-else :value="this.$data.alerts">
    <Column field="serverId" header="Server id" :sortable="true"></Column>
    <Column field="date" header="Date" :sortable="true"></Column>
    <Column field="text" header="Message" :sortable="true">
      <template #body="slotProps">
        <Tag :value="slotProps.data.text" :severity="getSeverity(slotProps.data.severity)"/>
      </template>
    </Column>
  </DataTable>
</template>

<style scoped>
.info{
  position: fixed;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  font-size: 64px;
}
</style>
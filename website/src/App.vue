<script>
import { Api } from "@/api";
import Menubar from "primevue/menubar";
import MegaMenu from "primevue/megamenu";
import {PrimeIcons} from "primevue/api";

export default {
  name: "app",
  data(){
    return{
      menu_items: [
        {
          label: 'Servers',
          icon: PrimeIcons.SERVER,
          items: []
        },
        {
          label: 'Alerts',
          icon: PrimeIcons.BELL,
          route: '/alerts'
        }
      ]
    }
  },
  components: {
    Menubar,
    MegaMenu
  },
  async created() {
    let servers = await Api.getServers()
    servers.forEach(s => {
      this.$data.menu_items[0].items.push({label: s.serverId, icon: PrimeIcons.SERVER, route: '/'})
    })
  }
};
</script>

<template>
  <nav style="margin-bottom: 20px">
    <div class="card">
      <Menubar :model="menu_items">
        <template #item="{item, props, hasSubmenu}">
          <router-link v-if="item.route" v-slot="{ href, navigate }" :to="item.route" custom>
            <a :href="href" v-bind="props.action" @click="navigate">
              <span :class="item.icon" style="margin-right: 10px;" />
              <span class="ml-2">{{ item.label }}</span>
            </a>
          </router-link>
          <a v-else :href="item.url" :target="item.target" v-bind="props.action">
            <span :class="item.icon" style="margin-right: 10px;" />
            <span class="ml-2">{{ item.label }}</span>
            <span v-if="hasSubmenu" class="pi pi-fw pi-angle-down ml-2" />
          </a>
        </template>
      </Menubar>
    </div>
  </nav>
  <main>
    <RouterView />
  </main>
</template>
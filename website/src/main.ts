import { createApp } from "vue";
import App from "./App.vue";
import { createMemoryHistory, createRouter } from 'vue-router'
import HomeView from './views/HomeView.vue'
import AlertView from "@/views/AlertView.vue";
import PrimeVue from "primevue/config";
import PrimeIcons from "primevue/config";
import "primeicons/primeicons.css";

const routes = [
  { path: '/:id', component: HomeView },
  { path: '/alerts', component: AlertView }
]

const router = createRouter({
  history: createMemoryHistory(),
  routes,
})

createApp(App).use(router).use(PrimeVue).use(PrimeIcons).mount("#app");

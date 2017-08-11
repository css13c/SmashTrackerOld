import { createApp } from "vue";
import App from "./App.vue";
import router from "./router";
import PrimeVue from "primevue/config";

const app = createApp(App).use(router).use(PrimeVue);

// Component Declarations

app.mount("#app");

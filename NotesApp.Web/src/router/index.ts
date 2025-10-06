import { createRouter, createWebHistory } from "vue-router";
import { useAuthStore } from "@/store/auth";

// Views (we’ll create them next)
import NoteList from "@/views/NoteList.vue";
import NoteDetail from "@/views/NoteDetail.vue";
import LoginView from "@/views/LoginView.vue";
import RegisterView from "@/views/RegisterView.vue";

const routes = [
  { path: "/", redirect: "/notes" },
  { path: "/notes", component: NoteList },
  { path: "/notes/new", component: NoteDetail },
  { path: "/notes/:id", component: NoteDetail },
  { path: "/login", component: LoginView },
  { path: "/register", component: RegisterView }
];

const router = createRouter({
  history: createWebHistory(),
  routes
});


// Protect notes route
router.beforeEach((to, from, next) => {
  const auth = useAuthStore();

  if (to.path.startsWith("/notes") && !auth.token) {
    next("/login");
  } else {
    next();
  }
});

export default router;

import { defineStore } from "pinia";
import { api } from "@/services/api";

interface User {
  id: number;
  username: string;
  email: string;
}

export const useAuthStore = defineStore("auth", {
  state: () => ({
    user: null as User | null,
    token: localStorage.getItem("token") || null,
    loading: false,
    error: null as string | null
  }),
  actions: {
    async fetchUserInfo() {
      try {
        const res = await api.get("/auth/me");
        this.user = res.data;
      } catch (err: any) {
        this.error = err.response?.data?.message || "Failed to fetch user info";
        this.logout();
      }
    },
    async login(username: string, password: string) {
      this.loading = true;
      try {
        const res = await api.post("/auth/login", { username, password });
        this.token = res.data.token;
        localStorage.setItem("token", this.token!);
        api.defaults.headers.common["Authorization"] = `Bearer ${this.token}`;
        // this.user = res.data.user;
        await this.fetchUserInfo(); // Fetch user info after successful login
        this.error = null;
      } catch (err: any) {
        this.error = err.response?.data?.message || "Login failed";
      } finally {
        this.loading = false;
      }
    },
    async register(username: string, email: string, password: string) {
      this.loading = true;
      try {
        const res = await api.post("/auth/register", { username, email, password });
        // this.token = res.data.token;
        // localStorage.setItem("token", this.token!);
        // api.defaults.headers.common["Authorization"] = `Bearer ${this.token}`;
        // this.user = res.data.user;
        // this.error = null;
        this.user = null;
        this.token = null;
        this.error = null;
      } catch (err: any) {
        this.error = err.response?.data?.message || "Registration failed";
      } finally {
        this.loading = false;
      }
    },
    async resetPassword(email: string, oldPassword: string, newPassword: string) {
      this.loading = true;
      try {
        const res = await api.post("/auth/reset-password", { email, oldPassword, newPassword });
        this.user = null;
        this.token = null;
        this.error = null;
      } catch (err: any) {
        this.error = err.response?.data?.message || "Reset failed";
      } finally {
        this.loading = false;
      }
    },
    logout() {
      this.user = null;
      this.token = null;
      localStorage.removeItem("token");
      delete api.defaults.headers.common["Authorization"];
    }
  }
});

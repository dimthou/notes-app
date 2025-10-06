import axios from "axios";

const baseURL = import.meta.env.VITE_API_URL || "http://localhost:5247/api";

// export const api = axios.create({
//   baseURL,
//   headers: { "Content-Type": "application/json" }
// });
export const api = axios.create({
  baseURL: baseURL,
  withCredentials: false
});

const token = localStorage.getItem("token");
if (token) {
  api.defaults.headers.common["Authorization"] = `Bearer ${token}`;
}

import { defineStore } from "pinia";
import { api } from "@/services/api";


export interface NoteListItem {
  id: number;
  title: string;
  createdAt: string;
}

export interface NoteDetail extends NoteListItem {
  content?: string;
  updatedAt: string;
}

export const useNotesStore = defineStore("notes", {
  state: () => ({
    notes: [] as NoteListItem[],
    selectedNote: null as NoteDetail | null,
    loading: false,
    searchQuery: "",
    sortBy: "createdAt" as "createdAt" | "title",
    sortDir: "desc" as "asc" | "desc"
  }),
  getters: {
    filteredNotes(state): NoteListItem[] {
      let result = [...state.notes];

      // Search
      if (state.searchQuery) {
        const q = state.searchQuery.toLowerCase();
        result = result.filter(
          n => n.title.toLowerCase().includes(q)
        );
      }

      // Sorting
      result.sort((a, b) => {
        let valA: string | Date = a[state.sortBy];
        let valB: string | Date = b[state.sortBy];
        if (state.sortBy === "createdAt") {
          valA = new Date(a.createdAt);
          valB = new Date(b.createdAt);
        }
        if (valA < valB) return state.sortDir === "asc" ? -1 : 1;
        if (valA > valB) return state.sortDir === "asc" ? 1 : -1;
        return 0;
      });

      return result;
    }
  },
  actions: {
    async fetchNotes() {
      this.loading = true;
      const res = await api.get("/notes");
      // Only keep id, title, createdAt for list
      this.notes = res.data.map((n: any) => ({
        id: n.id,
        title: n.title,
        createdAt: n.createdAt
      }));
      this.loading = false;
    },
    async fetchNoteDetail(id: number) {
      this.loading = true;
      const res = await api.get(`/notes/${id}`);
      this.selectedNote = res.data;
      this.loading = false;
    },
    clearSelectedNote() {
      this.selectedNote = null;
    },
    async createNote(title: string, content?: string) {
      // API expects: { title, content } (userId is from token)
      const res = await api.post("/notes", { title, content });
      // API returns the full note object
      const note = res.data;
      this.notes.unshift({
        id: note.id,
        title: note.title,
        createdAt: note.createdAt
      });
      this.selectedNote = note;
    },
    async updateNote(note: { id: number; title: string; content?: string }) {
      const res = await api.put(`/notes/${note.id}`, {
        title: note.title,
        content: note.content
      });
      // Update selectedNote
      this.selectedNote = res.data;
      // Update list
      const idx = this.notes.findIndex(n => n.id === note.id);
      if (idx !== -1 && this.notes[idx]) {
        this.notes[idx].title = res.data.title;
        this.notes[idx].createdAt = res.data.createdAt;
      }
    },
    async deleteNote(id: number) {
      await api.delete(`/notes/${id}`);
      this.notes = this.notes.filter(n => n.id !== id);
      if (this.selectedNote?.id === id) this.selectedNote = null;
    }
  }
});

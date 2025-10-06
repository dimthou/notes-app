

<script setup lang="ts">
import { onMounted, computed } from "vue";
import { useNotesStore } from "@/store/notes";
import { useRouter } from "vue-router";

const notesStore = useNotesStore();
const router = useRouter();

const sortOptions = [
  { value: "createdAt", label: "Date" },
  { value: "title", label: "Title" }
] as const;

function addNote() {
  router.push("/notes/new");
}

function handleDelete(id: number) {
  if (confirm("Are you sure you want to delete this note?")) {
    notesStore.deleteNote(id);
  }
}

function handleEdit(note: any) {
  router.push(`/notes/${note.id}`);
}

onMounted(() => {
  notesStore.fetchNotes();
});
</script>

<template>
  <div class="py-10">
    <header class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
      <div class="md:flex md:items-center md:justify-between">
        <div class="flex-1 min-w-0">
          <h2 class="text-2xl font-bold leading-7 text-gray-900 sm:text-3xl sm:truncate">My Notes</h2>
        </div>
        <div class="mt-4 flex md:mt-0 md:ml-4">
          <button
            @click="addNote"
            class="ml-3 inline-flex items-center px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
          >
            <svg class="-ml-1 mr-2 h-5 w-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4" />
            </svg>
            Add Note
          </button>
        </div>
      </div>
    </header>

    <main class="max-w-7xl mx-auto sm:px-6 lg:px-8">
      <!-- Search and Sort -->
      <div class="px-4 sm:px-0 mt-4">
        <div class="flex flex-col sm:flex-row gap-4">
          <div class="flex-1">
            <div class="relative">
              <div class="pointer-events-none absolute inset-y-0 left-0 flex items-center pl-3">
                <svg class="h-5 w-5 text-gray-400" viewBox="0 0 24 24" fill="none" stroke="currentColor">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
                </svg>
              </div>
              <input
                v-model="notesStore.searchQuery"
                type="search"
                placeholder="Search notes..."
                class="block w-full rounded-lg border-0 py-3 pl-10 pr-4 text-gray-900 ring-1 ring-inset ring-gray-300 placeholder:text-gray-400 focus:ring-2 focus:ring-inset focus:ring-blue-600 sm:text-sm sm:leading-6"
              />
              <div class="absolute inset-y-0 right-0 flex items-center pr-3">
                <span v-if="notesStore.searchQuery" class="text-sm text-gray-500">
                  Press Enter to search
                </span>
              </div>
            </div>
            <div v-if="notesStore.filteredNotes.length > 0" class="mt-2 text-sm text-gray-500">
              Found {{ notesStore.filteredNotes.length }} note(s)
            </div>
          </div>
          <div class="flex items-center gap-2">
            <select
              v-model="notesStore.sortBy"
              class="block w-full pl-3 pr-10 py-2 text-base border-gray-300 focus:outline-none focus:ring-blue-500 focus:border-blue-500 sm:text-sm rounded-md"
            >
              <option v-for="opt in sortOptions" :key="opt.value" :value="opt.value">
                Sort by {{ opt.label }}
              </option>
            </select>
            <button
              @click="notesStore.sortDir = notesStore.sortDir === 'asc' ? 'desc' : 'asc'"
              class="inline-flex items-center p-2 border border-gray-300 rounded-md text-gray-500 hover:text-gray-600 hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
              :title="'Sort ' + (notesStore.sortDir === 'asc' ? 'Ascending' : 'Descending')"
            >
              <svg 
                class="h-5 w-5" 
                :class="{ 'transform rotate-180': notesStore.sortDir === 'desc' }"
                fill="none" 
                stroke="currentColor" 
                viewBox="0 0 24 24"
              >
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 4h13M3 8h9m-9 4h6m4 0l4-4m0 0l4 4m-4-4v12" />
              </svg>
            </button>
          </div>
        </div>
      </div>

      <!-- Notes List -->
      <div class="mt-6">
        <ul class="space-y-3 sm:space-y-4">
          <li 
            v-for="note in notesStore.filteredNotes" 
            :key="note.id" 
            class="bg-white shadow-sm rounded-lg hover:shadow-md transition-shadow duration-200"
          >
            <div class="px-4 py-4 sm:px-6">
              <div class="flex items-center justify-between">
                <div class="flex-1 min-w-0">
                  <h3 class="text-lg font-medium text-gray-900 truncate">{{ note.title }}</h3>
                  <div class="mt-1 flex items-center text-sm text-gray-500">
                    <svg class="flex-shrink-0 mr-1.5 h-5 w-5 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
                    </svg>
                    <span>{{ new Date(note.createdAt).toLocaleString() }}</span>
                  </div>
                </div>
                <div class="flex gap-2">
                  <button
                    @click="handleEdit(note)"
                    class="inline-flex items-center px-3 py-1.5 border border-transparent text-sm font-medium rounded-md text-blue-700 bg-blue-100 hover:bg-blue-200 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
                  >
                    <svg class="-ml-0.5 mr-1 h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" />
                    </svg>
                    Edit
                  </button>
                  <button
                    @click="handleDelete(note.id)"
                    class="inline-flex items-center px-3 py-1.5 border border-transparent text-sm font-medium rounded-md text-red-700 bg-red-100 hover:bg-red-200 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-red-500"
                  >
                    <svg class="-ml-0.5 mr-1 h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                    </svg>
                    Delete
                  </button>
                </div>
              </div>
            </div>
          </li>
        </ul>
      </div>
    </main>
  </div>
</template>

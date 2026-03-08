// IndexedDB storage for KraftMesh Local Vault (offline cache).
// Exposes getItem/setItem for use via JS interop.

(function () {
    const DB_NAME = 'KraftMesh.Vault';
    const STORE_NAME = 'cache';
    const DB_VERSION = 1;

    let dbPromise = null;

    function openDb() {
        if (dbPromise) return dbPromise;
        dbPromise = new Promise((resolve, reject) => {
            const req = indexedDB.open(DB_NAME, DB_VERSION);
            req.onerror = () => reject(req.error);
            req.onsuccess = () => resolve(req.result);
            req.onupgradeneeded = (e) => {
                const db = e.target.result;
                if (!db.objectStoreNames.contains(STORE_NAME)) {
                    db.createObjectStore(STORE_NAME);
                }
            };
        });
        return dbPromise;
    }

    window.KraftMeshStorage = {
        getItem: function (key) {
            return openDb().then(function (db) {
                return new Promise((resolve, reject) => {
                    const tx = db.transaction(STORE_NAME, 'readonly');
                    const store = tx.objectStore(STORE_NAME);
                    const req = store.get(key);
                    req.onerror = () => reject(req.error);
                    req.onsuccess = () => resolve(req.result ?? null);
                });
            });
        },
        setItem: function (key, value) {
            return openDb().then(function (db) {
                return new Promise((resolve, reject) => {
                    const tx = db.transaction(STORE_NAME, 'readwrite');
                    const store = tx.objectStore(STORE_NAME);
                    const req = store.put(value, key);
                    req.onerror = () => reject(req.error);
                    req.onsuccess = () => resolve();
                });
            });
        }
    };

    window.KraftMeshStorage_getItem = function (key) { return window.KraftMeshStorage.getItem(key); };
    window.KraftMeshStorage_setItem = function (key, value) { return window.KraftMeshStorage.setItem(key, value); };
})();

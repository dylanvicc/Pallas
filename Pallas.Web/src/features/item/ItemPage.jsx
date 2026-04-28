import { useEffect, useState, useRef } from "react";
import ProtectedRoute from "../../app/ProtectedRoute";
import { itemService } from "../../services/itemService";
import ItemForm from "./ItemForm";
import ItemRow from "./ItemRow";

const EMPTY_FORM = { sku: "", name: "", description: "", manufacturer: "" };
const PAGE_SIZE = 25;

export default function ItemPage() {

    const [items, setItems] = useState([]);
    const [loading, setLoading] = useState(true);
    const [page, setPage] = useState(1);
    const [pageSize, setPageSize] = useState(PAGE_SIZE);
    const [totalCount, setTotalCount] = useState(0);
    const [search, setSearch] = useState("");
    const [searchInput, setSearchInput] = useState("");
    const [addOpen, setAddOpen] = useState(false);
    const [addForm, setAddForm] = useState(EMPTY_FORM);

    const totalPages = Math.ceil(totalCount / pageSize) || 1;

    const pageRef = useRef(page);
    const searchRef = useRef(search);
    const pageSizeRef = useRef(pageSize);
    pageRef.current = page;
    searchRef.current = search;
    pageSizeRef.current = pageSize;

    const loadItemsRef = useRef(null);
    loadItemsRef.current = async () => {
        setLoading(true);
        try {
            const data = await itemService.getItems({
                skip: (pageRef.current - 1) * pageSizeRef.current,
                take: pageSizeRef.current,
                search: searchRef.current,
            });
            setItems(data.data || []);
            setTotalCount(data.totalCount || 0);
            if (data.pageSize) setPageSize(data.pageSize);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        loadItemsRef.current();
    }, [page, search]);

    useEffect(() => {
        const timeout = setTimeout(() => {
            setPage(1);
            setSearch(searchInput);
        }, 300);
        return () => clearTimeout(timeout);
    }, [searchInput]);

    const handleEditRef = useRef();
    handleEditRef.current = async (id, updates) => {
        await itemService.updateItem(id, updates);
        await loadItemsRef.current();
    };

    const handleDeleteRef = useRef();
    handleDeleteRef.current = async (id) => {

        setItems((prev) => prev.filter((item) => item.id !== id));
        setTotalCount((prev) => prev - 1);

        try {
            await itemService.deleteItem(id);
        } catch (err) {
            await loadItemsRef.current();
        }
    };

    function handleAddChange(field, value) {
        setAddForm((prev) => ({ ...prev, [field]: value }));
    }

    async function handleAddSubmit() {

        if (!addForm.sku.trim() || !addForm.name.trim()) {
            return;
        }

        await itemService.createItem({ ...addForm, status: 0 });
        setAddForm(EMPTY_FORM);
        setAddOpen(false);
        await loadItemsRef.current();
    }

    return (
        <ProtectedRoute>
            <div className="max-w-4xl mx-auto p-8 space-y-4">
                <div className="flex items-baseline justify-between">
                    <h1 className="text-3xl font-semibold tracking-tight">Items</h1>
                    <span className="text-sm text-gray-400">{totalCount} items</span>
                </div>
                <div className="bg-gray-800 border border-gray-700 rounded-xl overflow-hidden">
                    <button
                        onClick={() => setAddOpen((o) => !o)}
                        className="w-full flex items-center justify-between px-5 py-3 text-sm font-medium text-white hover:bg-gray-700 transition-colors text-left"
                    >
                        <span>+ Add Item</span>
                        <span className={`text-gray-400 text-xs transition-transform duration-200 ${addOpen ? "rotate-180" : ""}`}>▼</span>
                    </button>
                    {addOpen && (
                        <div className="px-5 pb-5 pt-4 border-t border-gray-700">
                            <ItemForm
                                values={addForm}
                                onChange={handleAddChange}
                                onSubmit={handleAddSubmit}
                                onCancel={() => { setAddOpen(false); setAddForm(EMPTY_FORM); }}
                                submitLabel="Add Item"
                            />
                        </div>
                    )}
                </div>
                <div className="flex items-center gap-3">
                    <input
                        className="flex-1 px-4 py-2 bg-gray-800 border border-gray-700 rounded-lg text-sm text-white placeholder-gray-500 focus:outline-none focus:border-gray-500"
                        placeholder="Search"
                        value={searchInput}
                        onChange={(e) => setSearchInput(e.target.value)}
                    />
                    <span className="text-sm text-gray-400 whitespace-nowrap">
                        Page {page} of {totalPages}
                    </span>
                </div>
                <div className="bg-gray-800 border border-gray-700 rounded-xl divide-y divide-gray-700">
                    {loading ? (
                        <div className="p-6 text-sm text-gray-400">Loading…</div>
                    ) : items.length === 0 ? (
                        <div className="p-6 text-sm text-gray-400">No items found.</div>
                    ) : (
                        items.map((item) => (
                            <ItemRow
                                key={item.id}
                                item={item}
                                onEditRef={handleEditRef}
                                onDeleteRef={handleDeleteRef}
                            />
                        ))
                    )}
                </div>
                <div className="flex justify-between pt-2">
                    <button
                        disabled={page <= 1}
                        onClick={() => setPage((p) => p - 1)}
                        className="px-4 py-2 text-sm bg-gray-700 hover:bg-gray-600 text-white rounded-lg disabled:opacity-40 disabled:cursor-not-allowed"
                    >
                        Previous
                    </button>
                    <button
                        disabled={page >= totalPages}
                        onClick={() => setPage((p) => p + 1)}
                        className="px-4 py-2 text-sm bg-gray-700 hover:bg-gray-600 text-white rounded-lg disabled:opacity-40 disabled:cursor-not-allowed"
                    >
                        Next
                    </button>
                </div>
            </div>
        </ProtectedRoute>
    );
}
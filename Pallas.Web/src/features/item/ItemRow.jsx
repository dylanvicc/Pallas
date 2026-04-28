import { useState } from "react";
import ConfirmDialog from "../dialog/ConfirmDialog";
import ItemForm from "./ItemForm";

export default function ItemRow({ item, onEditRef, onDeleteRef }) {

    const [editing, setEditing] = useState(false);
    const [confirmDelete, setConfirmDelete] = useState(false);

    const [form, setForm] = useState({
        sku: item.sku,
        name: item.name,
        description: item.description || "",
        manufacturer: item.manufacturer || "",
    });

    function handleChange(field, value) {
        setForm((prev) => ({ ...prev, [field]: value }));
    }

    async function handleSave() {
        await onEditRef.current(item.id, form);
        setEditing(false);
    }

    function handleConfirmDelete() {
        setConfirmDelete(false);
        onDeleteRef.current(item.id);
    }

    return (
        <>
            {confirmDelete && (
                <ConfirmDialog
                    message={`Are you sure you want to delete ${item.name}? This action cannot be undone.`}
                    onConfirm={handleConfirmDelete}
                    onCancel={() => setConfirmDelete(false)}
                />
            )}
            <div className="p-5 border-b border-gray-700 last:border-0">
                {editing ? (
                    <ItemForm
                        values={form}
                        onChange={handleChange}
                        onSubmit={handleSave}
                        onCancel={() => setEditing(false)}
                        submitLabel="Save"
                    />
                ) : (
                    <div className="flex items-start justify-between gap-4">
                        <div className="space-y-1 min-w-0">
                            <div className="font-medium text-white">{item.name}</div>
                            <div className="text-sm text-gray-400">
                                SKU: {item.sku}
                                {item.manufacturer && ` · ${item.manufacturer}`}
                            </div>
                            {item.description && (
                                <div className="text-sm text-gray-300 mt-1">{item.description}</div>
                            )}
                        </div>
                        <div className="flex gap-2 shrink-0">
                            <button
                                onClick={() => setEditing(true)}
                                className="px-3 py-1 bg-yellow-600 hover:bg-yellow-700 text-white text-sm rounded-lg"
                            >
                                Edit
                            </button>
                            <button
                                onClick={() => setConfirmDelete(true)}
                                className="px-3 py-1 bg-red-600 hover:bg-red-700 text-white text-sm rounded-lg"
                            >
                                Delete
                            </button>
                        </div>
                    </div>
                )}
            </div>
        </>
    );
}
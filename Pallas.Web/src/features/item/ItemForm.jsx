export default function ItemForm({ values, onChange, onSubmit, onCancel, submitLabel }) {
    return (
        <div className="space-y-3">
            <div className="grid grid-cols-2 gap-3">
                <input
                    className="px-3 py-2 bg-gray-900 border border-gray-700 rounded-lg text-sm text-white placeholder-gray-500 focus:outline-none focus:border-gray-500"
                    placeholder="SKU"
                    value={values.sku}
                    onChange={(e) => onChange("sku", e.target.value)}
                />
                <input
                    className="px-3 py-2 bg-gray-900 border border-gray-700 rounded-lg text-sm text-white placeholder-gray-500 focus:outline-none focus:border-gray-500"
                    placeholder="Name"
                    value={values.name}
                    onChange={(e) => onChange("name", e.target.value)}
                />
                <input
                    className="col-span-2 px-3 py-2 bg-gray-900 border border-gray-700 rounded-lg text-sm text-white placeholder-gray-500 focus:outline-none focus:border-gray-500"
                    placeholder="Manufacturer"
                    value={values.manufacturer}
                    onChange={(e) => onChange("manufacturer", e.target.value)}
                />
                <textarea
                    className="col-span-2 px-3 py-2 bg-gray-900 border border-gray-700 rounded-lg text-sm text-white placeholder-gray-500 focus:outline-none focus:border-gray-500"
                    placeholder="Description"
                    rows={3}
                    value={values.description}
                    onChange={(e) => onChange("description", e.target.value)}
                />
            </div>
            <div className="flex gap-2">
                <button
                    onClick={onSubmit}
                    className="px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white text-sm rounded-lg"
                >
                    {submitLabel}
                </button>
                {onCancel && (
                    <button
                        onClick={onCancel}
                        className="px-4 py-2 bg-gray-700 hover:bg-gray-600 text-white text-sm rounded-lg"
                    >
                        Cancel
                    </button>
                )}
            </div>
        </div>
    );
}
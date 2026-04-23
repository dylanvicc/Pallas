import ProtectedRoute from "../../app/ProtectedRoute";

export default function InventoryPage() {

    const data = [
        {
            id: 1,
            item: "Widget A",
            location: "Warehouse 1",
            quantity: 120,
            reorderPoint: 50,
            reorderQty: 100,
            updatedAt: "2026-04-19",
        },
        {
            id: 2,
            item: "Widget B",
            location: "Warehouse 2",
            quantity: 50,
            reorderPoint: 30,
            reorderQty: 50,
            updatedAt: "2026-04-18",
        }
    ];

    return (
        <ProtectedRoute>
            <div className="max-w-6xl mx-auto">
                <h2 className="text-2xl font-semibold mb-6">Inventory Overview</h2>

                <div className="overflow-x-auto border border-gray-700 rounded-xl">
                    <table className="w-full text-left text-sm bg-gray-800">
                        <thead className="bg-gray-700 text-gray-300">
                            <tr>
                                <th className="px-4 py-3">Item</th>
                                <th className="px-4 py-3">Location</th>
                                <th className="px-4 py-3">Quantity</th>
                                <th className="px-4 py-3">Reorder Point</th>
                                <th className="px-4 py-3">Reorder Quantity</th>
                                <th className="px-4 py-3">Updated</th>
                            </tr>
                        </thead>

                        <tbody>
                            {data.map((row) => (
                                <tr
                                    key={row.id}
                                    className="border-t border-gray-700 hover:bg-gray-750 transition"
                                >
                                    <td className="px-4 py-3">{row.item}</td>
                                    <td className="px-4 py-3">{row.location}</td>
                                    <td className="px-4 py-3">{row.quantity}</td>
                                    <td className="px-4 py-3">{row.reorderPoint ?? "-"}</td>
                                    <td className="px-4 py-3">{row.reorderQty ?? "-"}</td>
                                    <td className="px-4 py-3">{row.updatedAt}</td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>
            </div>
        </ProtectedRoute>
    );
}

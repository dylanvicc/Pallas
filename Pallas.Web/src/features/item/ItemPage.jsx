import ProtectedRoute from "../../app/ProtectedRoute";
export default function ItemPage() {

    return (
        <ProtectedRoute>
            <div className="max-w-6xl mx-auto"></div>
        </ProtectedRoute>
    );
}

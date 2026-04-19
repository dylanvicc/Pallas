import LoginPage from "../features/login/LoginPage";
import InventoryPage from "../features/inventory/InventoryPage";

export default function Routes() {

    const path = window.location.pathname;

    if (path === "/login") return <LoginPage />;
    if (path === "/inventory") return <InventoryPage />;

    return (
        <div>
            <h2 className="text-3xl font-bold mb-4">Pallas Inventory Management System</h2>
        </div>
    );
}

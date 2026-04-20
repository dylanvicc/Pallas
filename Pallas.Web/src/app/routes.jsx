import LoginPage from "../features/login/LoginPage";
import InventoryPage from "../features/inventory/InventoryPage";
import ItemPage from "../features/item/ItemPage";
import LocationPage from "../features/location/LocationPage";

export default function Routes() {

    const path = window.location.pathname;

    if (path === "/login") return <LoginPage />;
    if (path === "/inventory") return <InventoryPage />;
    if (path === "/item") return <ItemPage />;
    if (path === "/location") return <LocationPage />;

    return (
        <div></div>
    );
}

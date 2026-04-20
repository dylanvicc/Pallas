import { useAuthentication } from "../features/login/AuthenticationContext";

export default function ProtectedRoute({ children }) {

    const { authentication } = useAuthentication();

    if (!authentication) {
        window.location.href = "/login";
        return null;
    }

    return children;
}
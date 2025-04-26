async function fetchWithToastr(url, options = {}) {
    try {
        const response = await fetch(url, options);

        if (response.status === 401) {
            window.location.href = '/Identity/Account/Login?returnUrl=' + encodeURIComponent(window.location.pathname);
            return null;
        }

        const data = await response.json();

        if (!response.ok) {
            let errorMessage = "An error occurred.";

            if (data?.errors) {
                // Take the first error we find
                const firstField = Object.keys(data.errors)[0];
                errorMessage = data.errors[firstField][0];
            }
            else if (data?.message) {
                errorMessage = data.message;
            }

            toastr.error(errorMessage, "", { timeOut: 5000, closeButton: true });
            return null;
        }


        return data;
    } catch (error) {
        console.error('Fetch Error:', error);
        toastr.error("Something went wrong. Please try again.", "", { timeOut: 5000, closeButton: true });
        return null;
    }
}
import { Separator } from '@/components/ui/separator';
import type { Product, ShoppingBasket } from '@/types';
import { useEffect, useMemo, type FC } from 'react';
import PlaceholderImage from '@/assets/placeholder-image.svg';
import { Button } from '@/components/ui/button';
import { Link, useNavigate } from 'react-router-dom';
import { useShoppingBasket } from '@/hooks/shopping-basket';


const PACKING_FEE = 4.51;
const DELIVERY_FEE = 10.99;
const SALES_TAX_PERCENT = 25;

const quantityTotal = (price: number, quantity: number): string => {
    const total = price * quantity;
    return total.toFixed(2);
};

const calcSubtotal = (shoppingBasket: ShoppingBasket): number => {
    return shoppingBasket.reduce((total, item) => {
        return total + item.product.price * item.quantity;
    }, 0);
};

const calcTotal = (price: number): number => {
    const withSalesTax = price * (1 + SALES_TAX_PERCENT / 100);
    return withSalesTax + PACKING_FEE + DELIVERY_FEE;
};

export const CheckoutPage: FC = () => {
    const navigate = useNavigate();
    const { basket } = useShoppingBasket();
    const subtotal: number = useMemo(() => {
        return calcSubtotal(basket);
    }, [basket]);

    useEffect(() => {
        if (basket.length <= 0) {
            void navigate('/');
        }
    }, [basket, navigate]);

    // Page for payment and delivery details
    return (  
        <div className="container mx-auto p-4">
            <h1 className="text-3xl font-bold mb-6">Checkout</h1>
            <div className="grid grid-cols-1 md:grid-cols-3 gap-8">
                <div className="md:col-span-2">
                    {/* User Details */}
                    <section className="mb-8">
                        <h2 className="text-2xl font-semibold mb-4">Delivery Details</h2>
                        <form className="space-y-4">
                            <div>
                                <label htmlFor="name" className="block text-sm font-medium text-gray-700">Full Name</label>
                                <input type="text" id="name" name="name" className="mt-1 block w-full p-2 border border-gray-300 rounded-md shadow-sm focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm" placeholder="John Doe" />
                            </div>
                            <div>
                                <label htmlFor="address" className="block text-sm font-medium text-gray-700">Address</label>
                                <input type="text" id="address" name="address" className="mt-1 block w-full p-2 border border-gray-300 rounded-md shadow-sm focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm" placeholder="123 Main St" />
                            </div>
                            <div>
                                <label htmlFor="city" className="block text-sm font-medium text-gray-700">City</label>
                                <input type="text" id="city" name="city" className="mt-1 block w-full p-2 border border-gray-300 rounded-md shadow-sm focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm" placeholder="Anytown" />
                            </div>
                            <div>
                                <label htmlFor="postalCode" className="block text-sm font-medium text-gray-700">Postal Code</label>
                                <input type="text" id="postalCode" name="postalCode" className="mt-1 block w-full p-2 border border-gray-300 rounded-md shadow-sm focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm" placeholder="12345" />
                            </div>
                            <div>
                                <label htmlFor="phone" className="block text-sm font-medium text-gray-700">Phone Number</label>
                                <input type="tel" id="phone" name="phone" className="mt-1 block w-full p-2 border border-gray-300 rounded-md shadow-sm focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm" placeholder="555-123-4567" />
                            </div>
                        </form>
                    </section>

                    <Separator className="my-8" />

                    {/* Payment Options */}
                    <section>
                        <h2 className="text-2xl font-semibold mb-4">Payment Information</h2>
                        <form className="space-y-4">
                            <div>
                                <label htmlFor="cardNumber" className="block text-sm font-medium text-gray-700">Card Number</label>
                                <input type="text" id="cardNumber" name="cardNumber" className="mt-1 block w-full p-2 border border-gray-300 rounded-md shadow-sm focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm" placeholder="**** **** **** ****" />
                            </div>
                            <div className="grid grid-cols-2 gap-4">
                                <div>
                                    <label htmlFor="expiryDate" className="block text-sm font-medium text-gray-700">Expiry Date</label>
                                    <input type="text" id="expiryDate" name="expiryDate" className="mt-1 block w-full p-2 border border-gray-300 rounded-md shadow-sm focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm" placeholder="MM/YY" />
                                </div>
                                <div>
                                    <label htmlFor="cvc" className="block text-sm font-medium text-gray-700">CVC</label>
                                    <input type="text" id="cvc" name="cvc" className="mt-1 block w-full p-2 border border-gray-300 rounded-md shadow-sm focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm" placeholder="123" />
                                </div>
                            </div>
                        </form>
                    </section>
                </div>

                {/* Order Summary */}
                <div className="md:col-span-1">
                    <div className="bg-gray-50 p-6 rounded-lg shadow">
                        <h2 className="text-2xl font-semibold mb-4">Order Summary</h2>
                        {basket.map((item) => (
                            <div key={item.product.id} className="flex justify-between items-center mb-2">
                                <div className="flex items-center">
                                    <img src={item.product.imageUrl || PlaceholderImage} alt={item.product.name} className="w-12 h-12 object-cover rounded mr-4" />
                                    <div>
                                        <p className="font-medium">{item.product.name}</p>
                                        <p className="text-sm text-gray-600">Qty: {item.quantity}</p>
                                    </div>
                                </div>
                                <p className="font-medium">${quantityTotal(item.product.price, item.quantity)}</p>
                            </div>
                        ))}
                        <Separator className="my-4" />
                        <div className="space-y-2">
                            <div className="flex justify-between">
                                <p>Subtotal</p>
                                <p>${subtotal.toFixed(2)}</p>
                            </div>
                            <div className="flex justify-between">
                                <p>Packing Fee</p>
                                <p>${PACKING_FEE.toFixed(2)}</p>
                            </div>
                            <div className="flex justify-between">
                                <p>Delivery Fee</p>
                                <p>${DELIVERY_FEE.toFixed(2)}</p>
                            </div>
                            <div className="flex justify-between">
                                <p>Sales Tax ({SALES_TAX_PERCENT}%)</p>
                                <p>${(subtotal * (SALES_TAX_PERCENT / 100)).toFixed(2)}</p>
                            </div>
                            <Separator className="my-4" />
                            <div className="flex justify-between text-xl font-bold">
                                <p>Total</p>
                                <p>${calcTotal(subtotal).toFixed(2)}</p>
                            </div>
                        </div>
                        <Button className="w-full mt-6" onClick={() => alert('Order Placed! (Not really)')}>
                            Place Order
                        </Button>
                        <Link to="/basket" className="block text-center mt-4">
                            <Button variant="outline" className="w-full">
                                Back to Basket
                            </Button>
                        </Link>
                    </div>
                </div>
            </div>
        </div>
    );
}   
const glob = require('glob');
const path = require('path');
const MiniCssExtractPlugin = require('mini-css-extract-plugin');

module.exports = {
    mode: 'development',
    entry: [
        './Client/Assets/Scripts/Test.ts',
        ...glob.sync('./Client/Assets/Scss/*.scss').map(file => `./${file}`)
    ],
    output: {
        filename: 'bundle.js',
        path: path.resolve(__dirname, './Client/wwwroot/js/dist'),
        library: '___MyScripts',
        libraryTarget: 'var',
    },
    module: {
        rules: [
            {
                test: /\.s[ac]ss$/i,
                use: [
                    MiniCssExtractPlugin.loader,
                    "css-loader",
                    "sass-loader",
                ],
            },
            {
                test: /\.ts$/,
                use: 'ts-loader',
                exclude: /node_modules/,
            },
            {
                test: /\.m?js$/,
                exclude: /(node_modules|bower_components)/,
                use: {
                    loader: 'babel-loader',
                    options: {
                        presets: ['@babel/preset-env'],
                    },
                },
            },
        ],
    },
    plugins: [
        new MiniCssExtractPlugin({
            filename: '../../css/style.css',
        }),
    ],
};